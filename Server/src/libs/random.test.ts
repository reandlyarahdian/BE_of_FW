import * as random from "./random";
import { expect } from "chai";
import sinon from "sinon";

describe("getRandomInt", () => {
  it("should return a number between min and max inclusive", () => {
    const min = 1;
    const max = 10;
    for (let i = 0; i < 100; i++) {
      const result = random.getRandomInt(min, max);
      expect(result).to.be.at.least(min);
      expect(result).to.be.at.most(max);
    }
  });

  it("should handle negative ranges", () => {
    const min = -10;
    const max = -1;
    for (let i = 0; i < 100; i++) {
      const result = random.getRandomInt(min, max);
      expect(result).to.be.at.least(min);
      expect(result).to.be.at.most(max);
    }
  });

  it("should return min when min and max are the same", () => {
    const min = 5;
    const max = 5;
    const result = random.getRandomInt(min, max);
    expect(result).to.equal(min);
  });
});

describe("random.getRandomMultiplier", () => {
  let getRandomIntStub: any;

  beforeEach(() => {
    getRandomIntStub = sinon.stub(Math, "random");
  });

  afterEach(() => {
    getRandomIntStub.restore();
  });

  it("should return 10000000 when random number is 0", () => {
    getRandomIntStub.returns(0);
    expect(random.getRandomMultiplier()).to.equal(10000000);
  });

  it("should return 1000000 when random number is between 1 and 9", () => {
    getRandomIntStub.returns(0.000005);
    expect(random.getRandomMultiplier()).to.equal(1000000);
  });

  it("should return 100000 when random number is between 10 and 99", () => {
    getRandomIntStub.returns(0.00005);
    expect(random.getRandomMultiplier()).to.equal(100000);
  });

  it("should return 10000 when random number is between 100 and 999", () => {
    getRandomIntStub.returns(0.0005);
    expect(random.getRandomMultiplier()).to.equal(10000);
  });

  it("should return 100 when random number is between 1000 and 9999", () => {
    getRandomIntStub.returns(0.005);
    expect(random.getRandomMultiplier()).to.equal(100);
  });

  it("should return 10 when random number is between 10000 and 99999", () => {
    getRandomIntStub.returns(0.05);
    expect(random.getRandomMultiplier()).to.equal(10);
  });

  it("should return 2 when random number is between 100000 and 499999", () => {
    getRandomIntStub.returns(0.3);
    expect(random.getRandomMultiplier()).to.equal(2);
  });

  it("should return 3 when random number is between 500000 and 799999", () => {
    getRandomIntStub.returns(0.7);
    expect(random.getRandomMultiplier()).to.equal(3);
  });

  it("should return 5 when random number is between 800000 and 999999", () => {
    getRandomIntStub.returns(0.9);
    expect(random.getRandomMultiplier()).to.equal(5);
  });

  it("should return 1 when random number is 1000000", () => {
    getRandomIntStub.returns(1);
    expect(random.getRandomMultiplier()).to.equal(1);
  });
});
