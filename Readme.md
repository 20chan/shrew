# Shrew

언어의 목표는 모든걸 프로시저로 생각하는 LISP와 비슷함. 모든 것을 패턴으로 생각하자

## 느낌적인느낌느낌

### Hello World

```
main = print "Hello World"
```

### Sum1To10

```
main = {
    sum = 0
    i = 1
    while i <= 10 {
        sum += i
    }
    print sum
}
```

### Fibonacci Sequence

```
// recursive
fib 0 = 1
fib 1 = 1
fib $n:int = fib $n-1 + fib $n-2
```

```
// iterative
fib $n:int = {
    a = 0
    b = 1
    loop $n {
        a, b = b, a + b
    }
    b
}
```

### Define `for`

```
for `( $init ; $cond ; $after; `) `{ $inner `} = {
        $init
        while $cond {
            $inner
            $after
        }
}
```

### Define Array, Range Syntax

```
`[ `] = array
`[ $first (`, $elem)* `] = array $first $elem*
// [1, 2, 3] = array 1 2 3
// [1, 1+1, 1] = array 1 (1+1) 1

$from `... $to = range $from $to
```

### Type

type 

## 구현

모든 expression 이 패턴에 있는지 확인해야 하니까 아마 트리와 탐색을 이용해서 패턴을 탐색하지 않을까 싶음. 바이트 코드 단위로 넘어가면 그냥 expression을 인덱싱하겠지 ?

세미콜론은 사실 넣고 싶지 않은데 그건 오토마타 설계에서 알아서 해야겠지...

## 타입

매개변수같은 경우는 `fun $n:int` 꼴로 타입 힌트를 줄 수 있음. 하지만 프로시저? 혹은 패턴 그 자체에 타입 힌트를 주려면 ??
`fun:int = 1` 같이 매개변수를 받지 않는 경우는 편하지만 `fun:int $n:int` 로 하면 이상하지 않을까?
않으면 정의 전에 하스켈처럼 선언할대 타입 힌트를 줄 수 있게 할까?
```
fun:int
fun 3 2 = "hi" // Compile error!
fun $a $b = 3
```