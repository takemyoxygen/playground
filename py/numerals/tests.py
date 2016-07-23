import numerals
import functools

test_cases = {
    123: "CXXIII",
    1: "I",
    10: "X",
    14: "XIV",
    4999: "MMMMCMXCIX",
    270: "CCLXX",
    787: "DCCLXXXVII",
    1231: "MCCXXXI"
}


def test_arabic_to_roman_conversion():
    for (arabic, roman) in test_cases.items():
        yield functools.partial(check, numerals.convert_arabic_to_roman), arabic, roman


def test_roman_to_arabic_conversion():
    for (arabic, roman) in test_cases.items():
        yield functools.partial(check, numerals.convert_roman_to_arabic), roman, arabic


def check(converter, input_value, expected):
    assert expected == converter(input_value)
