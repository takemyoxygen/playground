(ns advent-of-code-2016.day11)

(defn floor-to-index
  [floor]
  (case
    floor
    "first" :1
    "second" :2
    "third" :3
    "fourth" :4))

(defn parse-item
  [item]
  (let [[_ material _ type] (re-matches #"a (.+?)(-compatible)? (generator|microchip)" item)]
    {:material material :type type}))

(defn parse-content
  [content]
  (if (= content "nothing relevant")
    []
    (let [items (clojure.string/split content #", (and )?| and ")]
      (map parse-item items))))

(defn parse-line
  [line]
  (let [[_ floor content-string] (re-matches #"The (.*) floor contains (.*)." line)
        index (floor-to-index floor)
        content (parse-content content-string)]
    {index content}))

(defn parse-input
  [raw-input]
  (apply merge (map parse-line (clojure.string/split-lines raw-input))))