def alphabet_position(text):
    offset = ord('a') - 1
    positions = (ord(char) - offset for char in text.lower() if char.isalpha())
    return ' '.join(map(str, positions))
