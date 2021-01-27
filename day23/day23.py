import re
from math import log2, pow

def load_input():
    rr = re.compile(r".*<(-?\d+),(-?\d+),(-?\d+)>, r=(\d+)")

    f = open("./day23-input.txt", "r")
    #f = open("./day23-input-test.txt", "r")
    #f = open("./day23-input-test2.txt", "r")
    input = f.readlines()

    largestrange = 0
    nanobots = []
    for line in [l.strip() for l in input]:
        match = rr.search(line)
        nanobots.append([int(x) for x in match.groups()])
    return nanobots


def main():
    nanobots = load_input()
    nanobots.sort(key=lambda x: x[3], reverse=True)
    orgX, orgY, orgZ, orgR = nanobots[0]

    part1 = 0
    for n in nanobots:
        dist = abs(orgX - n[0]) + abs(orgY - n[1]) + abs(orgZ - n[2])
        if (dist <= orgR):
            part1 += 1
    print("Part 1:", part1)

    # Part 2
    # With much help from reddit after working out that the first attempt was
    # going to take at least 450 hours
    # https://www.reddit.com/r/adventofcode/comments/a8s17l/2018_day_23_solutions/
    # especially this https://pastebin.com/e7LdSNQe , which, to be fair, it looks
    # very similar to, oddly enough
    # First attempt was so very close but tried to loop through
    # all x,y,z (ie 1000*1000*1000 calcs) for each bot - ie waaay too many
    # all it needed was the divide and conquer approach - reduce the target area
    # and number of calcs bit by bit. Remarkable difference in speed!
    xx = [n[0] for n in nanobots]
    yy = [n[1] for n in nanobots]
    zz = [n[2] for n in nanobots]

    # Divide and conquer
    rng = max(xx) - min(xx)
    step = int(pow(2, int(log2(rng))))
    # or, perhaps
    # step = 1
    # while step < rng:
    #     step *= 2
    # step = step // 2

    while True:
        bestnum = 0
        bestmd = 0
        bestpos = None
        for x in range(min(xx), max(xx) + 1, step):
            for y in range(min(yy), max(yy) + 1, step):
                for z in range(min(zz), max(zz) + 1, step):
                    inrange = 0
                    for nx, ny, nz, nr in nanobots:
                        dist = abs(x - nx) + abs(y - ny) + abs(z - nz)
                        if dist - step < nr:
                            inrange += 1

                    if inrange > 0 and inrange >= bestnum:
                        thismd = abs(x) + abs(y) + abs(z)
                        if inrange == bestnum:
                            if bestmd <= 0 or thismd < bestmd:
                                bestmd = thismd
                                bestpos = (x, y, z)
                        else:
                            bestnum = inrange
                            bestmd = thismd
                            bestpos = (x, y, z)
        # Reduce the range centred around our best so far
        if step == 1:
            part2 = bestmd
            break
        else:
            xx = [bestpos[0] - step, bestpos[0] + step]
            yy = [bestpos[1] - step, bestpos[1] + step]
            zz = [bestpos[2] - step, bestpos[2] + step]
            step = step // 2

    print("Part 2:", part2)

if __name__ == "__main__":
    main()
