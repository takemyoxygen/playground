def order(sentence: str):
    words_with_numbers = ((word, next(c for c in word if c.isdigit())) for word in sentence.split())
    sorted_words = sorted(words_with_numbers, key=lambda x: x[1])
    return " ".join(x[0] for x in sorted_words)



print(order("is2 Thi1s T4est 3a"))
