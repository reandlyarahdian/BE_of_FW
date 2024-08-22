import { HX_SECRET } from "../constant";
import { fetchApi, HeadersAuth, HeadersAuthWithAdmin } from "../libs/fetch-api";

type EnergyAction = 'decrement' | 'increment' | 'set';

export class Energy {
    private headers: HeadersAuthWithAdmin;
    private gameId;

    constructor(headers: HeadersAuth, gameId: string) {
        this.gameId = gameId;
        this.headers = {
            ...headers,
            hx_secret: HX_SECRET
        };
    }

    private updateEnergy(action: EnergyAction, value: number) {
        return fetchApi
            .post(
                `/v1/queue/sync/game/${this.gameId}`,
                {
                    data: {
                        energy: {
                            [action]: value
                        }
                    }
                },
                { headers: this.headers }
            )
            .then(response => response.data);
    }

    decrement(value: number) {
        return this.updateEnergy('decrement', value);
    }

    increment(value: number) {
        return this.updateEnergy('increment', value);
    }

    set(value: number) {
        return this.updateEnergy('set', value);
    }
}