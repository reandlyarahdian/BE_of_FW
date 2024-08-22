import { HX_SECRET } from "../constant";
import { fetchApi, HeadersAuth, HeadersAuthWithAdmin } from "../libs/fetch-api";

type MelonPointsAction = 'decrement' | 'increment' | 'set';

export class MelonPoints {
    private headers: HeadersAuthWithAdmin;
    private gameId;

    constructor(headers: HeadersAuth, gameId: string) {
        this.gameId = gameId;
        this.headers = {
            ...headers,
            hx_secret: HX_SECRET
        };
    }

    private updateMelonPoints(action: MelonPointsAction, value: number) {
        return fetchApi
            .post(
                `/v1/queue/sync/game/${this.gameId}`,
                {
                    data: {
                        melon: {
                            [action]: value
                        }
                    }
                },
                { headers: this.headers }
            )
            .then(response => response.data);
    }

    decrement(value: number) {
        return this.updateMelonPoints('decrement', value);
    }

    increment(value: number) {
        return this.updateMelonPoints('increment', value);
    }

    set(value: number) {
        return this.updateMelonPoints('set', value);
    }
}