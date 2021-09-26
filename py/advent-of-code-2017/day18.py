from collections import defaultdict, deque
from typing import Union, List, Callable
import operator

Operand = Union[str, int]


class State:
    def __init__(self, program=0):
        self.position = 0
        self.played_sound = 0
        self.recovered_sound = 0
        self.registers = defaultdict(lambda: 0)
        self.registers['p'] = program
        self.queue = deque()
        self.counterpart = None
        self.sent_count = 0


def eval_operand(operand: Operand, registers: dict[str, int]) -> int:
    try:
        return int(operand)
    except ValueError:
        return registers[operand]


def modify_register(modification: Callable[[int, int], int]):
    def apply_modification(state: State, register: str, value: Operand):
        state.registers[register] = modification(state.registers[register], eval_operand(value, state.registers))
        state.position += 1

    return apply_modification


def perform_sound(state: State, value: Operand):
    state.played_sound = eval_operand(value, state.registers)
    state.position += 1


def perform_recover(state: State, value: Operand):
    if eval_operand(value, state.registers) != 0:
        state.recovered_sound = state.played_sound
    state.position += 1


def perform_jgz(state: State, condition: Operand, offset: Operand):
    if eval_operand(condition, state.registers) > 0:
        state.position += eval_operand(offset, state.registers)
    else:
        state.position += 1


def perform_send(state: State, value: Operand):
    state.counterpart.queue.append(eval_operand(value, state.registers))
    state.position += 1
    state.sent_count += 1


def perform_receive(state: State, value: Operand):
    if state.queue:
        state.registers[value] = state.queue.popleft()
        state.position += 1


instruction_handlers_1 = {
    'set': modify_register(lambda old, new: new),
    'add': modify_register(operator.add),
    'mul': modify_register(operator.mul),
    'mod': modify_register(operator.mod),
    'snd': perform_sound,
    'rcv': perform_recover,
    'jgz': perform_jgz
}


instruction_handlers_2 = instruction_handlers_1.copy()
instruction_handlers_2['snd'] = perform_send
instruction_handlers_2['rcv'] = perform_receive


def perform_instruction(state: State, instruction: List[str], handlers: dict[str, Callable]):
    [name, *args] = instruction
    handlers[name](state, *args)


def part1(instructions: List[List[str]]) -> int:
    state = State()
    while 0 <= state.position < len(instructions) and state.recovered_sound == 0:
        perform_instruction(state, instructions[state.position], instruction_handlers_1)
    return state.recovered_sound


def part2(instructions: List[List[str]]) -> int:
    p0 = State(0)
    p1 = State(1)
    p0.counterpart = p1
    p1.counterpart = p0
    p0_before, p1_before = None, None
    while p0_before != p0.position or p1_before != p1.position:
        if 0 <= p0.position < len(instructions):
            p0_before = p0.position
            perform_instruction(p0, instructions[p0.position], instruction_handlers_2)
        if 0 <= p1.position < len(instructions):
            p1_before = p1.position
            perform_instruction(p1, instructions[p1.position], instruction_handlers_2)
    return p1.sent_count


instructions = [line.rstrip('\n').split(' ') for line in open('input/day18.txt').readlines()]

print('Part 1:', part1(instructions))
print('Part 2:', part2(instructions))