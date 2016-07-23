from functools import reduce
import operator

arabic_to_roman = {
    1: "I",
    5: "V",
    10: "X",
    50: "L",
    100: "C",
    500: "D",
    1000: "M"}

roman_to_arabic = {roman: arabic for (arabic, roman) in arabic_to_roman.items()}

MAX = 4999


def unfold(f, initial_state):
    next_item, state = f(initial_state)
    yield next_item
    while state:
        next_item, state = f(state)
        yield next_item


def summands_of(x):

    def unfolder(remaining, power):
        return ((remaining % 10), power), \
               (remaining // 10, power + 1) if remaining > 9 else None

    return (x for x in unfold(lambda state: unfolder(*state), (x, 0)) if x != 0)


def summand_to_roman(digit, power):
    multiplier = 10 ** power
    lower_roman = arabic_to_roman[multiplier]
    medium_roman = arabic_to_roman[multiplier * 5] if multiplier * 5 in arabic_to_roman else None
    high_roman = arabic_to_roman[multiplier * 10] if multiplier * 10 in arabic_to_roman else None
    if digit <= 3 or not medium_roman:
        return lower_roman * digit
    elif digit <= 8:
        return max(5 - digit, 0) * lower_roman + medium_roman + max(digit - 5, 0) * lower_roman
    else:
        return lower_roman + high_roman


def convert_arabic_to_roman(number):
    if number > MAX:
        raise Exception("Only values smaller than {} are supported".format(MAX))

    summands = sorted(summands_of(number), key=lambda x: x[1], reverse=True)
    summands_in_roman = (summand_to_roman(digit, power) for (digit, power) in summands)

    return reduce(operator.concat, summands_in_roman)