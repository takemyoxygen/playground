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

fn build_alphabet() -> HashMap<char, [&'static str; 3]>{
    let mut alphabet = HashMap::with_capacity(10);
    alphabet.insert('0', [" _ ",
                           "| |",
                           "|_|"]);

    alphabet.insert('1', [" ",
                           "|",
                           "|"]);

    alphabet.insert('2', [" _ ",
                           " _|",
                           "|_ "]);

    alphabet.insert('3', ["_ ",
                           "_|",
                           "_|"]);

    alphabet.insert('4', ["   ",
                           "|_|",
                           "  |"]);

    alphabet.insert('5', [" _ ",
                           "|_ ",
                           " _|"]);

    alphabet.insert('6', [" _ ",
                           "|_ ",
                           "|_|"]);

    alphabet.insert('7', ["_ ",
                           " |",
                           " |"]);

    alphabet.insert('8', [" _ ",
                           "|_|",
                           "|_|"]);

    alphabet.insert('9', [" _ ",
                           "|_|",
                           " _|"]);

    alphabet
}

fn main() {
    let alphabet = build_alphabet();
    let input = read_input();
    print!("You've entered: {}", input);

    let to_print: Vec<_> = input
        .trim()
        .chars()
        .map(|c| match alphabet.get(&c){
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
