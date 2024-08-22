export const scorePerActionAvgCheck = (score: number, actionCount: number, threshold: number) => {
    const scorePerActionAvg = score/actionCount;

    if (scorePerActionAvg > threshold) {
        return false;
    }

    return true;
}