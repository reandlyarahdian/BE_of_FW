export class PlaySessionLogs {
    private username: string;
    private game_id: string;
    private history: any[] = [];

    constructor(username: string, game_id: string) {
        this.username = username;
        this.game_id = game_id;
    }

    addHistory(data: any) {
        this.history.push(data);
    }

    showLog(message: string) {
        console.log(`======
username: ${this.username}
game: ${this.game_id}
history: ${JSON.stringify(this.history, null, 2)}

${message}
======`.trim()
        )
    }
}