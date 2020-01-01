(ns advent-of-code-2016.monorail)

(defn- parse-operand
  [operand]
  (if (nil? operand)
    nil
    (try
      {:type :constant :value (Integer/parseInt operand)}
      (catch NumberFormatException _
        {:type :register :name operand}))))

(defn- parse-line
  [input-line]
  (let [[operation operand1 operand2] (clojure.string/split input-line #"\s")]
    {:name operation :operands (remove nil? [(parse-operand operand1) (parse-operand operand2)])}))

(defn parse-instructions
  "Parses a string containing a set of instructions into a sequence of instruction maps"
  [input]
  (map parse-line (clojure.string/split-lines input)))

(defn register?
  [{:keys [type]}]
  (= type :register))

(defn constant?
  [{:keys [type]}]
  (= type :constant))

(defn get-empty-state
  [instructions]
  {:next 0 :instructions (vec instructions) :registers {}})

(defn resolve-value
  [operand state]
  (if (= (:type operand) :constant)
    (:value operand)
    (get-in state [:registers (:name operand)] 0)))

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


(defn toggle-instruction
  [{:keys [name operands] :as instruction}]
  (cond
    (and (= 1 (count operands)) (= "inc" name)) {:name "dec" :operands operands}
    (= 1 (count operands)) {:name "inc" :operands operands}
    (and (= 2 (count operands)) (= "jnz" name)) {:name "cpy" :operands operands}
    (= 2 (count operands)) {:name "jnz" :operands operands}
    :else instruction))

(defn perform-tgl
  [{[operand] :operands} {:keys [instructions] :as state}]
  (let [instruction-index (+ (:next state) (resolve-value operand state))
        new-instructions (if
                           (contains? instructions instruction-index)
                           (update instructions instruction-index toggle-instruction)
                           instructions)]
    (-> state
        (assoc :instructions new-instructions)
        (move-next))))

(defn perform-cpy
  [{[op1 op2] :operands} state]
  (if
    (register? op2)
    (->> state
         (set-register op2 (resolve-value op1 state))
         move-next)
    (move-next state)))

(defn perform-inc
  [{[op] :operands} state]
  (if
    (register? op)
    (->> state
         (update-register op inc)
         move-next)
    (move-next state)))

(defn perform-dec
  [{[op] :operands} state]
  (if
    (register? op)
    (->> state
         (update-register op dec)
         move-next)
    (move-next state)))

(defn perform-mult
  [{[op1 op2] :operands} state]
  (let [result (* (resolve-value op1 state) (resolve-value op2 state))]
    (->> state
         (set-register op1 result)
         (move-next))))

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
    "tgl" (perform-tgl operation state)
    "mult" (perform-mult operation state)
    (throw (Exception. (str "Unknown operation" operation)))))

(defn execute-instructions
  [state]
  (loop [{:keys [instructions next] :as current-state} state]
    (if (contains? instructions next)
      (recur (perform-operation (instructions next) current-state))
      current-state)))