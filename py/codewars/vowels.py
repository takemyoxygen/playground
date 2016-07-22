def vowel_indices(word):
    vowels = {'a', 'e', 'i', 'o', 'u', 'y'}
    return [i + 1 for (i, c) in enumerate(word) if c.lower() in vowels]


print(vowel_indices("Mmmm"))
print(vowel_indices("Super"))
print(vowel_indices("Apple"))
print(vowel_indices("YoMama"))