import { Schema, Context, type, MapSchema } from "@colyseus/schema";
export class PlaySession extends Schema {
  @type("string") playSessionId: string;
  @type("number") playSessionStartTS: number;
  @type("number") playSessionActionCount: number;
  @type("boolean") isEligibleForMultiplier: boolean;
  @type("number") multiplierQuota: number = 1;
  // playSessionError: string;
}