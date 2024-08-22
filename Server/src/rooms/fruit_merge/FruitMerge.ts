import { Room, Client } from "@colyseus/core";
import { FruitMergeState } from "./schema/FruitMergeState";
import { getRandomInt, getRandomMultiplier } from "../../libs/random";
import { scorePerActionAvgCheck } from "../../libs/standard-checks";
import crypto from "crypto";
import { PlaySession } from "../../schema/PlaySessionState";
import { multiplierPosition } from "../../libs/multiplier-position";
import { HeadersAuth } from "../../libs/fetch-api";
import { Energy } from "../../api/energy";
import { User } from "../../api/user";
import { MelonPoints } from "../../api/melon-points";
import { PlaySessionLogs } from "../../api/playsession-logs";
import { NODE_ENV } from "../../constant";

export class FruitMerge extends Room<FruitMergeState> {
  private headerAuth: HeadersAuth;
  private GAME_ID = "fruit-merge";
  private energy_required = 8;
  private energy: Energy;
  private user: User;
  private melonPoints: MelonPoints;
  private previous_score = 0;
  private sessionLog: PlaySessionLogs;

  private async initSyncDb(): Promise<UserDetail> {
    this.user = new User(this.headerAuth);
    this.energy = new Energy(this.headerAuth, this.GAME_ID);
    this.melonPoints = new MelonPoints(this.headerAuth, this.GAME_ID);

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

    return user  // return user data, can be access with client.auth;;
  }

  onCreate(options: any) {
    this.setState(new FruitMergeState());
    this.state.playSession = new PlaySession();
    this.state.energy = 1;
    this.maxClients = 1;

    this.onMessage("start_playsession", (client, message) => {
      this.sessionLog.addHistory(`energy: ${this.state.energy}`);
      if (this.state.energy < this.energy_required) {
        this.sessionLog.addHistory("NEE: NotEnoughEnergy");
      } else {
        this.state.energy -= this.energy_required;

        this.state.playSession.playSessionId = crypto.randomUUID();
        this.state.playSession.playSessionStartTS = Date.now();
        this.state.playSession.playSessionActionCount = 0;

        this.state.nextFruit = 1;
        this.state.score = 0; //

        // store to db
        this.energy.decrement(this.energy_required);
      }
    });

    this.onMessage("shuffle_next_fruit", (client, message) => {
      this.state.nextFruit = getRandomInt(1, 4);
      this.state.playSession.playSessionActionCount++;
    });

    this.onMessage("set_score", (client, message) => {
      if (parseInt(message) < 100000) {
        this.state.score = parseInt(message);
      }

      if (
        !scorePerActionAvgCheck(
          this.state.score,
          this.state.playSession.playSessionActionCount,
          250
        )
      ) {
        this.sessionLog.addHistory("cheating alert!");
        this.disconnect();
      }
      this.sessionLog.addHistory(`score: ${this.state.score}`);
    });

    this.onMessage("get_current_energy", (client, message) => {
      client.send("current_energy", this.state.energy);
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
      this.energy.increment(this.energy_required)
    })

    this.onMessage("multiply_points", (client, message) => {
      this.sessionLog.addHistory("clicked! muliply point");

      if (this.state.playSession.isEligibleForMultiplier) {
        this.state.playSession.isEligibleForMultiplier = false;

        const multiplier = getRandomMultiplier();
        client.send("multiplier", multiplier);
        client.send("multiplier_pos", multiplierPosition(multiplier));

        this.sessionLog.addHistory(`${multiplier}x`);

        this.state.score *= multiplier;

        this.sessionLog.addHistory(`score: ${this.state.score}`);
      }
    });

    this.onMessage("end_playsession", (client, message) => {
      this.state.playSession = new PlaySession();
      // store to db
      if (this.state.score) {
        this.previous_score = this.state.score;
        this.melonPoints.increment(this.state.score);
      }
    });
  }

  onJoin(client: Client, options: any) {
    this.sessionLog.showLog("joined!");
    // send current energy
    client.send("current_energy", this.state.energy);
  }

  onLeave(client: Client, consented: boolean) {
    this.sessionLog.showLog("left!");
    // store to db
    if (this.state.score && this.state.score !== this.previous_score) {
      this.previous_score = this.state.score;
      this.melonPoints.increment(this.state.score);
    }
  }

  onDispose() {
    // console.log("room", this.roomId, "disposing...");
  }
}
