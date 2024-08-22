import { Room, Client } from "@colyseus/core";
import { KingHitState } from "./schema/KingHitState";
import { getRandomInt, getRandomMultiplier } from "../../libs/random";
import { scorePerActionAvgCheck } from "../../libs/standard-checks";
import crypto from "crypto";
import { PlaySession } from "../../schema/PlaySessionState";
import { multiplierPosition } from "../../libs/multiplier-position";

export class KingHit extends Room<KingHitState> {
  onCreate(options: any) {
    this.setState(new KingHitState());
    this.state.playSession = new PlaySession();
    this.state.energy = 8;

    this.onMessage("start_playsession", (client, message) => {
      console.log(this.state.energy);
      if (this.state.energy < 8) {
        console.log("NEE: NotEnoughEnergy");
        client.error(2, "NEE: NotEnoughEnergy");
      } else {
        this.state.energy = this.state.energy - 8;

        this.state.playSession.playSessionId = crypto.randomUUID();
        this.state.playSession.playSessionStartTS = Date.now();
        this.state.playSession.playSessionActionCount = 0;

        this.state.score = 0;
      }
    });

    this.onMessage("set_score", (client, message) => {
      this.state.playSession.playSessionActionCount++;
      if (parseInt(message) < 100000) {
        this.state.score = parseInt(message);
      }

      if (
        !scorePerActionAvgCheck(
          this.state.score,
          this.state.playSession.playSessionActionCount,
          30
        )
      ) {
        console.log("cheating alert!");
        this.disconnect();
      }
      console.log(this.state.score);
    });

    this.onMessage("get_current_energy", (client, message) => {
      console.log(this.state.energy);
      client.send("current_energy", this.state.energy);
    });

    this.onMessage("multiplier_watch_ads_finished", (client, message) => {
      // check callback
      if (this.state.playSession.multiplierQuota > 0) {
        this.state.playSession.multiplierQuota -= 1;
        this.state.playSession.isEligibleForMultiplier = true;
      }
    });

    this.onMessage("energy_watch_ads_finished", (client, message) => {
      // check callback
      this.state.energy += 8;
    });

    this.onMessage("multiply_points", (client, message) => {
      console.log("clicked!");

      if (this.state.playSession.isEligibleForMultiplier) {
        this.state.playSession.isEligibleForMultiplier = false;

        const multiplier = getRandomMultiplier();
        // const multiplier = 100.;
        client.send("multiplier", multiplier);
        client.send("multiplier_pos", multiplierPosition(multiplier));
        console.log(multiplier, "x");

        this.state.score *= multiplier;

        console.log(this.state.score);
      }
    });

    this.onMessage("end_playsession", (client, message) => {
      this.state.playSession = new PlaySession();
      // store to db
    });
  }

  onJoin(client: Client, options: any) {
    console.log(client.sessionId, "joined!");
  }

  onLeave(client: Client, consented: boolean) {
    console.log(client.sessionId, "left!");
  }

  onDispose() {
    console.log("room", this.roomId, "disposing...");
  }
}
