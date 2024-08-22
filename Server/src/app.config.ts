import config from "@colyseus/tools";
import { monitor } from "@colyseus/monitor";
import { playground } from "@colyseus/playground";

/**
 * Import your Room files
 */
import { FruitMerge } from "./rooms/fruit_merge/FruitMerge";
import { StackingColors } from "./rooms/staking_colors/StackingColors";
import { KingHit } from "./rooms/king_hit/KingHit";
import { MatchTile } from "./rooms/match_tile/MatchTile";
import { GAME_SERVER_API_KEY } from "./constant";

export default config({

    initializeGameServer: (gameServer) => {
        /**
         * Define your room handlers:
         */
        gameServer.define('fruit_merge', FruitMerge);
        gameServer.define('stacking_colors', StackingColors);
        gameServer.define('king_hit', KingHit);
        gameServer.define('match_tile', MatchTile);
    },

    initializeExpress: (app) => {
        /**
         * Bind your custom express routes here:
         * Read more: https://expressjs.com/en/starter/basic-routing.html
         */
        // app.get("/hello_world", (req, res) => {
        //     res.send("It's time to kick ass and chew bubblegum!");
        // });

        app.post("/rewarded_ads_ssv_cb", (req, res) => {

        })

        /**
         * Use @colyseus/playground
         * (It is not recommended to expose this route in a production environment)
         */
        if (process.env.NODE_ENV !== "production") {
            app.use("/", playground);
        }

        /**
         * Use @colyseus/monitor
         * It is recommended to protect this route with a password
         * Read more: https://docs.colyseus.io/tools/monitor/#restrict-access-to-the-panel-using-a-password
         */
        app.use("/colyseus", monitor());

        app.get("/logs", (req, res) => {
            const key = req.query.key;

            if (!key || key !== GAME_SERVER_API_KEY) {
                return res.status(401).send("Unauthorized");
            }

            const filePath = '/home/deploy/.pm2/logs/colyseus-app-out-0.log';
            res.download(filePath, (err) => {
                if (err) {
                    console.error(`Error sending the file: ${err}`);
                    res.status(500).send('An error occurred while trying to download the file.');
                }
            });
        })
    },


    beforeListen: () => {
        /**
         * Before before gameServer.listen() is called.
         */
    }
});
