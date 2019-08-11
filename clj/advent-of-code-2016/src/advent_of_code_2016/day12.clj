(ns advent-of-code-2016.day12)

(defn parse-operand
  [operand]
  (if (nil? operand)
    nil
    (try
      {:type :constant :value (Integer/parseInt operand)}
      (catch NumberFormatException _
        {:type :register :name operand}))))

(defn parse-line
  [input-line]
  (let [[operation operand1 operand2] (clojure.string/split input-line #"\s")]
    {:name operation :operands (remove nil? [(parse-operand operand1) (parse-operand operand2)])}))

(defn parse-input
  [input]
  (map parse-line (clojure.string/split-lines input)))

(defn initial-state
  [operations]
  {:next 0 :operations (vec operations) :registers {"a" 0 "b" 0 "c" 0 "d" 0}})

(defn resolve-value
  [operand state]
  (if (= (:type operand) :constant)
      (:value operand)
      (get-in state [:registers (:name operand)])))

(defn move-next
  [state]
  (update state :next inc))

(defn set-register
  [register value state]
  (assoc-in state [:registers (:name register)] value))

(defn update-register
  [register f state]
  (let [current (resolve-value register state)]
    (set-register register (f current) state)))

(defn perform-cpy
  [{[op1 op2] :operands} state]
  (->> state
       (set-register op2 (resolve-value op1 state))
       move-next))

(defn perform-inc
  [{[op] :operands} state]
  (->> state
       (update-register op inc)
       move-next))

(defn perform-dec
  [{[op] :operands} state]
  (->> state
       (update-register op dec)
       move-next))

(defn perform-jnz
  [{[op steps] :operands} state]
  (let [steps-value (resolve-value steps state)
        registry-value (resolve-value op state)]
    (if (= registry-value 0)
      (move-next state)
      (update state :next (partial + steps-value)))))

(defn perform-operation
  [operation state]
  (case (:name operation)
    "cpy" (perform-cpy operation state)
    "inc" (perform-inc operation state)
    "dec" (perform-dec operation state)
    "jnz" (perform-jnz operation state)
    (throw (Exception. (str "Unknown operation" operation)))))

(defn execute-operations
  [state]
  (loop [current-state state]
    (if (> (count (:operations current-state)) (:next current-state))
      (recur (perform-operation ((:operations current-state) (:next current-state)) current-state))
      current-state)))

(defn solve-part-1
  [input]
  (let [instructions (parse-input input)
        state (initial-state instructions)]
    (execute-operations state)))

(defn solve-part-2
  [input]
  (let [instructions (parse-input input)
        state (->> (initial-state instructions)
                   (set-register {:name "c"} 1))]
    (execute-operations state)))