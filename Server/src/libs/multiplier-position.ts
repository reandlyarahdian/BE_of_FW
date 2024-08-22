import { getRandomInt } from "./random";

export const multiplierPosition = (multiplier: number) => {
    let selection;
    const selectionIdx = getRandomInt(0, 1)

    switch (true) {
        case multiplier == 2:
            selection = selectionIdx == 0? 4 : 8
            return selection
        case multiplier == 3:
            selection = selectionIdx == 0? 3 : 7
            return selection
        case multiplier == 5:
            selection = selectionIdx == 0? 2 : 6
            return selection
        case multiplier > 5:
            selection = selectionIdx == 0? 1 : 5
            return selection
        default:
            return 0
    }
}