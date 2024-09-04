import { Room, Client } from "@colyseus/core";
import { MatchTileState } from "./schema/MatchTileState";
import { scorePerActionAvgCheck } from "../../libs/standard-checks";
import crypto from "crypto";
import { PlaySession } from "../../schema/PlaySessionState";
import { HeadersAuth } from "../../libs/fetch-api";
import { Energy } from "../../api/energy";
import { User } from "../../api/user";
import { Level } from "../../api/level";
import { MelonPoints } from "../../api/melon-points";
import { PlaySessionLogs } from "../../api/playsession-logs";
import { NODE_ENV } from "../../constant";

export class MatchTile extends Room<MatchTileState> {
  private headerAuth: HeadersAuth;
  private GAME_ID = "match-tile";
  private energy_required = 8;
  private energy: Energy;
  private user: User;
  private melonPoints: MelonPoints;
  private previous_score = 0;  
  private previous_lvl = 0;
  private sessionLog: PlaySessionLogs;
  private level: Level;

  private async initSyncDb(): Promise<UserDetail> {
    this.user = new User(this.headerAuth);
    this.energy = new Energy(this.headerAuth, this.GAME_ID);
    this.melonPoints = new MelonPoints(this.headerAuth, this.GAME_ID);
    this.level = new Level(this.headerAuth, this.GAME_ID);

    const user = await this.user.getDetail();

    if (NODE_ENV === "local") {
      this.sessionLog = new PlaySessionLogs("local", this.GAME_ID);
    } else {
      this.sessionLog = new PlaySessionLogs(user.username, this.GAME_ID);
    }

    return user;
  }

  async onAuth(client: Client, options: any) {
    if (NODE_ENV === 'local') {
      await this.initSyncDb();
      return {};
    }

    const { token, appKey } = options;

    this.headerAuth = { "app-pub-key": appKey, Authorization: `Bearer ${token}` } // passing header auth;

    const user = await this.initSyncDb();

    this.state.energy = user.points.energy;
    this.state.currentLevel = user.points.level.currentLevel;
    this.state.score = user.points.melon;

    return user  // return user data, can be access with client.auth;;
  }

  onCreate(options: any) {
    this.setState(new MatchTileState());
    this.state.playSession = new PlaySession();
    this.state.energy = 16;
    this.maxClients = 1;
    this.state.currentLevel = 2;
    this.state.score = 30;

    this.onMessage("request_initial_data", (client, message) => {
      client.send("initial_data", {
        currentLevel: this.state.currentLevel,
        score: this.state.score,
        energy: this.state.energy
      });
    });

    this.onMessage("start_playsession", (client, message) => {
      this.sessionLog.showLog("start!");
      this.sessionLog.addHistory(`energy: ${this.state.energy}`);
      if (this.state.energy < this.energy_required) {
        this.sessionLog.addHistory("NEE: NotEnoughEnergy");
      } else {
        //this.state.energy -= this.energy_required;

        this.state.playSession.playSessionId = crypto.randomUUID();
        this.state.playSession.playSessionStartTS = Date.now();
        this.state.playSession.playSessionActionCount = 0; //
        this.sessionLog.showLog(this.state.playSession.playSessionActionCount.toString());
        // store to db
        this.energy.set(this.state.energy);
        }
    });

    this.onMessage("decreese_energy", (client, message) =>{
      this.state.energy -= this.energy_required;
      this.energy.decrement(this.energy_required);     
      this.sessionLog.addHistory(`energy: ${this.state.energy}`);  
    })

    this.onMessage("set_score", (client, message : number) => {
      this.sessionLog.showLog("score!");
      if (message < 100000) {
        this.state.score = message;
      }

      if (isNaN(message) || message < 0) {
        this.sessionLog.addHistory("Invalid score received: " + message);
        return;
      }

      if (
        !scorePerActionAvgCheck(
          this.state.score,
          this.state.playSession.playSessionActionCount,
          1000
        )
      ) {
        this.sessionLog.addHistory("cheating alert!");
        this.disconnect();
      }
      const score = message;
        client.send("score", score);

        this.sessionLog.addHistory(`${score}`);

        this.state.score = score;

        this.sessionLog.addHistory(`score: ${this.state.score}`);
    });

    this.onMessage("set_lvl", (client, message : number) => {
      this.sessionLog.showLog("level!");
      if (message < 100000) {
        this.state.currentLevel = message;
      }
      if (isNaN(message) || message < 0) {
        this.sessionLog.addHistory("Invalid level received: " + message);
        return;
      }
      if (
        !scorePerActionAvgCheck(
          this.state.currentLevel,
          this.state.playSession.playSessionActionCount,
          201
        )
      ) {
        this.sessionLog.addHistory("cheating alert!");
        this.disconnect();
      }
      const level = message;
      client.send("level", level);

      this.sessionLog.addHistory(`${level}`);

      this.state.currentLevel = level;

      this.sessionLog.addHistory(`level: ${this.state.currentLevel}`);
    });

    this.onMessage("get_current_energy", (client, message) => {
      this.sessionLog.addHistory(`energy: ${this.state.energy}`);  
      client.send("energy", this.state.energy);
    });

    this.onMessage("add_energy", (client, message) => {
      this.state.energy += 1;
      this.sessionLog.addHistory(`energy: ${this.state.energy}`);
    });

    this.onMessage("multiplier_watch_ads_finished", (client, message) => {
      // check callback
      if (this.state.playSession.multiplierQuota > 0) {
        this.state.playSession.multiplierQuota -= 1;
        this.state.playSession.isEligibleForMultiplier = true;
      }
    })

    this.onMessage("energy_watch_ads_finished", (client, message) => {
      // check callback
      this.state.energy += this.energy_required;
      // store to db
      this.energy.increment(this.energy_required);

      this.sessionLog.addHistory(`energy: ${this.state.energy}`);  
    })

    this.onMessage("wheels_points", (client, message : number) => {
      this.sessionLog.addHistory("clicked! add point");
      this.sessionLog.showLog("wheels!");

      const wheels = message;
        client.send("wheels", wheels);

        this.sessionLog.addHistory(`${wheels}`);

        this.state.score += wheels;

        this.sessionLog.addHistory(`score: ${this.state.score}`);
    });

    this.onMessage("multiply_points", (client, message : number) => {
      this.sessionLog.addHistory("clicked! multiply point");
      this.sessionLog.showLog("multiply!");
      if (this.state.playSession.isEligibleForMultiplier) {
        this.state.playSession.isEligibleForMultiplier = false;

        const multiplier = message;
        client.send("multiplier", multiplier);

        this.sessionLog.addHistory(`${multiplier}x`);

        this.state.score *= multiplier;

        this.sessionLog.addHistory(`score: ${this.state.score}`);
      }
    });

    this.onMessage("end_playsession", (client, message) => {
      this.state.playSession = new PlaySession();
      this.sessionLog.showLog("end!");
      // store to db
      if (this.state.score) {
        this.previous_score = this.state.score;
        this.melonPoints.decrement(this.state.score);
      }

      if (this.state.currentLevel) {
        this.previous_lvl = this.state.currentLevel;
        this.level.setCurrentLevel(this.state.currentLevel);
      }
    });
  }

  onJoin(client: Client, options: any) {
    this.sessionLog.showLog("joined!");
    
    client.send("level", this.state.currentLevel);
    // send current energy
    client.send("score", this.state.score);

    client.send("energy", this.state.energy);
  }

  onLeave(client: Client, consented: boolean) {
    this.sessionLog.showLog("left!");
    // store to db
    if (this.state.score && this.state.score !== this.previous_score) {
      this.previous_score = this.state.score;
      this.melonPoints.decrement(this.state.score);
    }
    if (this.state.currentLevel 
      && this.state.currentLevel !== this.previous_lvl) {
      this.previous_lvl = this.state.currentLevel;
      this.level.setCurrentLevel(this.state.currentLevel);
    }
  }

  onDispose() {
    // console.log("room", this.roomId, "disposing...");
  }
}
