from flask import Flask
import numerals

app = Flask("Numerical converter")


@app.route("/")
def home():
    return "Hello from converter"


@app.route("/<arabic>/roman")
def to_roman(arabic):
    print("Converting {} to roman".format(arabic))
    converted = numerals.convert_arabic_to_roman(int(arabic))
    print("Conversion result: ", converted)

    return converted


@app.route("/<roman>/arabic")
def to_arabic(roman):
    print("Converting {} to arabic".format(roman))
    converted = numerals.convert_roman_to_arabic(roman)
    print("Conversion result: ", converted)

    return str(converted)


if __name__ == "__main__":
    app.run()