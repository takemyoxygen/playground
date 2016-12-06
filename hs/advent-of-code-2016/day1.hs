import Data.List.Split

data State = State { direction :: Int -- 0 - North, 1 - East, 2 - South, 3 - West
                   , movedNorth :: Int
                   , movedEast :: Int 
                   } deriving (Show)

data Turn = L | R deriving (Show, Eq)

data Move = Move Turn Int

nextState :: State -> Move -> State
nextState state (Move turn blocks) =
    let nextDirection = ((if turn == L then -1 else 1) + (direction state)) `mod` 4
        (northDelta, eastDelta) = case nextDirection of 0 -> (1, 0)
                                                        1 -> (0, 1)
                                                        2 -> (-1, 0)
                                                        3 -> (0, -1)
    in State nextDirection (movedNorth state + northDelta * blocks) (movedEast state + eastDelta * blocks)

solve :: [Move] -> State
solve moves = foldl nextState (State 0 0 0) moves

distance :: State -> Int
distance state = abs (movedNorth state) + abs (movedEast state)

parse :: String -> [Move]
parse input = map parseMove $ splitOn ", " input where 
    parseMove (dir:length) = Move (if dir == 'L' then L else R) $ read length

main = do 
    putStrLn "Input:"
    input <- getLine
    let parsed = parse input
    let final = solve parsed
    putStrLn $ "Final state:" ++ (show final)
    putStrLn $ "Distance: " ++ (show $ distance final)