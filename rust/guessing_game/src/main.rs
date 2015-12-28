extern crate rand;

use std::io;
use std::cmp::Ordering;
use rand::Rng;

fn generate() -> i32 {
    rand::thread_rng().gen_range(1, 101)
}

fn as_int(input: &String) -> i32 {
    input.trim().parse()
        .ok()
        .expect("That's not a number, dude")
}

fn ask(secret: &i32) -> Ordering {
    println!("Enter your guess");

    let mut guess = String::new();
    io::stdin().read_line(&mut guess)
        .ok()
        .expect("Failed to read line");

    as_int(&guess).cmp(&secret)
}

fn ask_in_loop(secret: &i32) {
    let comparison = ask(&secret);

    match comparison {
        Ordering::Less    => {
            println!("Too small!");
            ask_in_loop(secret);
        },
        Ordering::Greater => {
            println!("Too big!");
            ask_in_loop(secret);
        },
        Ordering::Equal   => println!("You win!"),
    }
}

fn main() {
    let secret = generate();
    println!("Guess the numba! (Very secret number is {})", secret);
    ask_in_loop(&secret);
}
