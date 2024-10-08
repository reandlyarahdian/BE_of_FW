import { Schema, Context, type, MapSchema } from "@colyseus/schema";
import { PlaySession } from "../../../schema/PlaySessionState";

export class StackingColorsState extends Schema {
  @type(PlaySession) playSession: PlaySession;

  @type("number") energy: number;
  @type("number") score: number;
}
