interface UserDetail {
    address_wallet: string
    email: string
    username: any
    points: UserPoints
}

interface UserPoints {
    energy: number
    lpx: number
    roulette: number
    melon: number
    experience: number
    level: UserLevel
}

interface UserLevel {
    currentLevel: number
    requiredExp: number
    currentExp: number
    expToNextLevel: number
}
