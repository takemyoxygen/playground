#[derive(Debug, Eq, PartialEq)]
enum Color {
    Red,
    Black
}

#[derive(Eq, PartialEq, Debug)]
enum Card {
    King,
    Ace,
    Two,
    Three
}

impl Card {
    fn all() -> Vec<Card> {
        vec![Card::King, Card::Ace, Card::Two, Card::Three]
    }
}

fn round(red: &Card, black: &Card) -> Color {
    match (red, black) {
        (&Card::King, &Card::King) => Color::Red,
        (_, &Card::King) | (&Card::King, _) => Color::Black,
        _ if red != black => Color::Red,
        _ => Color::Black
    }
}

fn pairwise<T>(items: &Vec<T>) -> Vec<(&T, &T)> {
    items
        .iter()
        .flat_map(|x| items.iter().map(move |y| (x, y)))
        .collect()
}

fn main() {
    let cards = Card::all();
    let pairs = pairwise(&cards);

    let red_wins = pairs
        .iter()
        .map(|&(r, b)| round(r, b))
        .filter(|c| *c == Color::Red)
        .count();

    let black_wins = pairs.len() - red_wins;
    println!("Red wins in {} cases, black wins in {} cases", red_wins, black_wins);

}
