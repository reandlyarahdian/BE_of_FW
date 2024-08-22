  // Generate a random integer between min (inclusive) and max (inclusive)
export const getRandomInt = (min: number, max: number) => {
  min = Math.ceil(min);
  max = Math.floor(max);

  return Math.floor(Math.random() * (max - min + 1)) + min;
};

export const getRandomMultiplier = () => {    
    const randomInteger = getRandomInt(0, 1000000);

    switch (true) {
      // case randomInteger < 1:
      //   return 10000000;
      // case randomInteger < 10:
      //   return 1000000;
      case randomInteger < 100:
        return 100000;
      case randomInteger < 1000:
        return 10000;
      case randomInteger < 10000:
        return 100;
      case randomInteger < 100000:
        return 10;
      case randomInteger < 500000:
        return 2;
      case randomInteger < 800000:
        return 3;
      case randomInteger < 1000000:
        return 5;
      default:
        return 1;
    }
}