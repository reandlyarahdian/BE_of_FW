import axios from 'axios';
import axiosRetry from 'axios-retry';
import Agent from 'agentkeepalive';
import { ENPOINT_API_BACKEND, NODE_ENV } from '../constant';
const { registerInterceptor } = require('axios-cached-dns-resolve');

export type HeadersAuth = {
    Authorization: string,
    "app-pub-key": string
}

export type HeadersAuthWithAdmin = HeadersAuth & { "hx_secret": string }

const keepAliveConfig = {
    maxSockets: 160,
    maxFreeSockets: 160,
    timeout: 60000,
    freeSocketTimeout: 30000,
    keepAliveMsecs: 60000
}

const keepAliveAgent = new Agent(keepAliveConfig);
const httpsKeepAliveAgent = new Agent.HttpsAgent(keepAliveConfig);

const fetchInstance = axios.create({
    baseURL: ENPOINT_API_BACKEND,
    httpAgent: keepAliveAgent,
    httpsAgent: httpsKeepAliveAgent
})

// Interceptor Response
fetchInstance.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        if (NODE_ENV === 'local') {
            return Promise.resolve({});
        }
        return Promise.reject(error);
    }
);

registerInterceptor(fetchInstance);
axiosRetry(fetchInstance, { retries: 3 });

export const fetchApi = fetchInstance;