import re

pattern = r"(-?\d+),(-?\d+),(-?\d+),(-?\d+)"

def load_input(fname):
    rr = re.compile(pattern)
    f = open(fname, "r")
    result = []
    for line in f.readlines():
        m = rr.search(line)
        result.append([int(x) for x in m.groups()])
    return result

def get_constellation_count(points):
    result = 0

    allocated = []
    while True:
        current = []
        for point in points:
            if point in allocated:
                continue
            current.append(point)
            allocated.append(point)
            break
        if len(current) == 0: # no more left
            return result

        more = True
        while more:
            anynew = False
            for point in points:
                if point in current or point in allocated:
                    continue
                # is it in range of the current constellation
                inrange = False
                for c in current:
                    dist = abs(c[0] - point[0]) + abs(c[1] - point[1]) + abs(c[2] - point[2]) + abs(c[3] - point[3])
                    if dist <= 3:
                        inrange = True
                        anynew = True
                        break
                if inrange:
                    allocated.append(point)
                    current.append(point)
            if not anynew:
                result += 1
                more = False



def main():
    points = load_input("./day25-input.txt")
    #points = load_input("./day25-input-test.txt")
    #points = load_input("./day25-input-test-a.txt")
    part1 = get_constellation_count(points)
    print("Part 1:", part1, "constellations")


if __name__ == "__main__":
    main()