from flask import Flask
import numerals

app = Flask("Numerical converter")


@app.route("/")
def home():
    return "Hello from converter"


@app.route("/<number>/roman")
def to_roman(number):
    return numerals.convert_arabic_to_roman(int(number))


if __name__ == "__main__":
    app.run()