# AoC 2018 day 1, parts 1 and 2

f = open("./day01-part1.txt", "r")
input = f.readlines()
f.close()

# Part 1
part1 = sum(int(line) for index, line in enumerate(input))
print("Part 1:", part1)

# Part 2
seen = set()
found = False
part2 = 0
while not found:
    for _, c in enumerate(input):
        part2 += int(c)

        if part2 in seen:
            found = True
            break
        # else
        seen.add(part2)

print("Part 2", part2)
