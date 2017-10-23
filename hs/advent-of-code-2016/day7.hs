import Data.Char (isLetter)

type IPv7 = String

data TlsCheckResult = SupportsTls | DoesNotSupportTls String deriving (Show, Eq)

supportsTls :: IPv7 -> TlsCheckResult
supportsTls ip = 
    let ipLength = length ip
        isAbba index = 
            ((index + 3) < ipLength) && 
            isLetter (ip !! index) &&
            isLetter (ip !! (index + 1)) &&
            (ip !! index /= ip !! (index + 2)) &&
            (ip !! index == ip !! (index + 3)) && 
            (ip !! (index + 1) == ip !! (index + 2))

        loop index bracketsDepth hasAbbaOutsideOfHypernet =
            case ip !! index of
            _ | index == ipLength -> 
                if hasAbbaOutsideOfHypernet 
                then SupportsTls 
                else DoesNotSupportTls "Doesn't have ABBA record"
            '[' -> loop (index + 1) (bracketsDepth + 1) hasAbbaOutsideOfHypernet
            ']' -> loop (index + 1) (bracketsDepth - 1) hasAbbaOutsideOfHypernet
            _ | isAbba index -> 
                if bracketsDepth == 0 
                then loop (index + 1) bracketsDepth True 
                else DoesNotSupportTls "There's an ABBA record inside []"
            _  -> loop (index + 1) bracketsDepth hasAbbaOutsideOfHypernet

    in loop 0 0 False

solve :: [IPv7] -> Int
solve ips = length $ filter (\ip -> supportsTls ip == SupportsTls) ips

countOfIpsWithTls :: [(IPv7, TlsCheckResult)] -> Int
countOfIpsWithTls = length . filter (\(_, result) -> result == SupportsTls)

solveTest :: [IPv7] -> [(IPv7, TlsCheckResult)]
solveTest = map (\ip -> (ip, supportsTls ip))

main :: IO ()
main = do
    ips <- lines <$> getContents
    let tlsSupportInfo = solveTest ips
    let count = countOfIpsWithTls tlsSupportInfo
    mapM_ print tlsSupportInfo
    print count