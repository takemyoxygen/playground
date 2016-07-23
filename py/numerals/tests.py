import numerals


test_cases = {
    123: "CXXIII",
    1: "I",
    10: "X",
    14: "XIV",
    4999: "MMMMCMXCIX",
    270: "CCLXX",
    787: "DCCLXXXVII"
}


def test_conversion():
    for (arabic, roman) in test_cases.items():
        yield check, arabic, roman


def check(arabic, expected):
    assert expected == numerals.convert_arabic_to_roman(arabic)
