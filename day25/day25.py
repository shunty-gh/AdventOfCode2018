import re

pattern = r"(-?\d+),(-?\d+),(-?\d+),(-?\d+)"

def load_input(fname):
    rr = re.compile(pattern)
    f = open(fname, "r")
    result = []
    for line in f.readlines():
        m = rr.search(line)
        v = [int(x) for x in m.groups()]
        v.append(False) # add a boolean to indicate if this item has been allocated into a constellation
        result.append(v)
    return result

def get_constellation_count(points):
    result = 0

    while True:
        current = []
        start = next((p for p in points if not p[4]), None)
        if start == None: # no more left
            return result
        start[4] = True  # mark as allocated
        current.append(start)

        more = True
        # continually iterate through to find all points in range
        while more:
            anynew = False
            for point in points:
                if point[4]: # already allocated
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
                    point[4] = True # mark as allocated
                    current.append(point)

            if not anynew:   # nothing new found this time through
                result += 1  # so increment the result
                more = False # and start a new constellation



def main():
    points = load_input("./day25-input.txt")
    #points = load_input("./day25-input-test.txt")
    #points = load_input("./day25-input-test-a.txt")
    part1 = get_constellation_count(points)
    print("Part 1:", part1, "constellations")


if __name__ == "__main__":
    main()