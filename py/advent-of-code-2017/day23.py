from typing import List
from collections import defaultdict
import pprint

from common.registers import handlers, Registers


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


def format_registers(registers: Registers) -> str:
    return ', '.join(map(lambda key: f'{key}: {registers[key]}', registers))


def part2(instructions: List[List[str]], max_steps=None) -> int:
    registers = defaultdict(lambda: 0)
    registers['a'] = 1
    position = 0
    steps = 0
    while 0 <= position < len(instructions) and (max_steps is None or steps < max_steps):
        [name, *args] = instructions[position]
        position = handlers[name](registers, position, *args)
        steps += 1
        print(f'Step #{steps} after "{name} {" ".join(args)}", position: {position}, registers: {format_registers(registers)}')
    return registers['h']


instructions = [line.rstrip('\n').split(' ') for line in open('input/day23.txt').readlines()]

# print('Part 1:', part1(instructions))
print('Part 2:', part2(instructions, 100))
