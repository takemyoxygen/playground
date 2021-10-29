from typing import List
from collections import defaultdict

from common.registers import handlers


def part1(instructions: List[List[str]]) -> int:
    registers = defaultdict(lambda: 0)
    mul_count = 0
    position = 0
    while 0 <= position < len(instructions):
        [name, *args] = instructions[position]
        if name == 'mul':
            mul_count += 1

        position = handlers[name](registers, position, *args)

    return mul_count


instructions = [line.rstrip('\n').split(' ') for line in open('input/day23.txt').readlines()]
print('Part 1:', part1(instructions))