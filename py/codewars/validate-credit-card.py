def validate(n):
    digits = list(map(int, str(n)))
    for i in range(len(digits) - 2, -1, -2):
        digits[i] = digits[i] * 2 if digits[i] < 5 else digits[i] * 2 - 9
    return sum(digits) % 10 == 0

print(validate(12345))
