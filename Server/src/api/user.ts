import { fetchApi, HeadersAuth } from "../libs/fetch-api"

export class User {
    private headers: HeadersAuth;

    constructor(headers: HeadersAuth) {
        this.headers = headers;
    }

    getDetail() {
        return fetchApi.get('/v1/user/detail', { headers: this.headers }).then(d => d.data as UserDetail);
    }
    
    getLevel(){
        return fetchApi.get('/v1/user/detail/points/level', { headers: this.headers }).then(d => d.data as UserLevel);
    }
}