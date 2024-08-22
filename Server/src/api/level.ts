import { HX_SECRET } from "../constant";
import { fetchApi, HeadersAuth, HeadersAuthWithAdmin } from "../libs/fetch-api";

type LevelAction = 'currentLevel';
type LevelStatusAction = 'set';

export class Level {
    private headers: HeadersAuthWithAdmin;
    private gameId;

    constructor(headers: HeadersAuth, gameId: string) {
        this.gameId = gameId;
        this.headers = {
            ...headers,
            hx_secret: HX_SECRET
        };
    }

    private updateLevel(action: LevelAction, status: LevelStatusAction, value: number) {
        return fetchApi
            .post(
                `/v1/queue/sync/game/${this.gameId}`,
                {
                    data: {
                        level: {
                            [action] : {
                                [status] : value
                            }
                        }
                    }
                },
                { headers: this.headers }
            )
            .then(response => response.data);
    }

    setCurrentLevel(value: number) {
        return this.updateLevel('currentLevel', 'set', value);
    }
}