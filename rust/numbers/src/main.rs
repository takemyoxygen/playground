use std::io;
use std::collections::HashMap;

fn read_input() -> String {
    println!("Enter the number");

    let mut input = String::new();
    io::stdin().read_line(&mut input)
        .ok()
        .expect("Failed to read the line from the console");

    input
}

fn main() {
    let mut encodings = HashMap::new();
    encodings.insert('0', [" _ ",
                           "| |",
                           "|_|"]);

    encodings.insert('1', [" ",
                           "|",
                           "|"]);

    encodings.insert('2', [" _ ",
                           " _|",
                           "|_ "]);

    encodings.insert('3', ["_ ",
                           "_|",
                           "_|"]);

    encodings.insert('4', ["   ",
                           "|_|",
                           "  |"]);

    encodings.insert('5', [" _ ",
                           "|_ ",
                           " _|"]);

    encodings.insert('6', [" _ ",
                           "|_ ",
                           "|_|"]);

    encodings.insert('7', ["_ ",
                           " |",
                           " |"]);

    encodings.insert('8', [" _ ",
                           "|_|",
                           "|_|"]);

    encodings.insert('9', [" _ ",
                           "|_|",
                           " _|"]);

    let input = read_input();
    print!("You've entered: {}", input);

    let to_print: Vec<_> = input
        .trim()
        .chars()
        .map(|c| match encodings.get(&c){
            Some(encoded) => encoded,
            None => panic!("Character {} is not known", &c)
        })
        .collect();

    for i in 0..3 {
        for s in &to_print {
            print!("{} ", s[i]);
        }
        println!("");
    }
}
