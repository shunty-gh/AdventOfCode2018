# AoC 2018 day 5

f = open("./day05-part1.txt", "r")
input = f.read()
f.close()

def react(source):
    result = []
    for c in source:
        prev = c if len(result) == 0 else result[len(result) - 1]
        if (prev == c) or prev.upper() != c.upper():
            result.append(c)
        else:
            result.pop()
    return "".join(result)

# Part 1
part1 = react(input)
print("Part 1:", len(part1))

# Part 2
part2 = min([len(react(part1.replace(c, "").replace(c.lower(), ""))) for c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ"])
print("Part 2:", part2)
