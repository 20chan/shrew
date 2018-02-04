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
fib $n:int = fib ($n-1) + fib ($n-2)
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

for (i = 0; i<=10; i++) {
    print i
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

```
type Person(name:string, age:int)

a = Person("0xad", 3)
a.name = a.name + ";"
print a.name // "0xad;"
```

## 구현

모든 expression 이 패턴에 있는지 확인해야 하니까 아마 트리와 탐색을 이용해서 패턴을 탐색하지 않을까 싶음. 바이트 코드 단위로 넘어가면 그냥 expression을 인덱싱하겠지 ?

세미콜론은 사실 넣고 싶지 않은데 그건 오토마타 설계에서 알아서 해야겠지...

## 타입 힌트

매개변수같은 경우는 `fun $n:int` 꼴로 타입 힌트를 줄 수 있음. 하지만 프로시저? 혹은 패턴 그 자체에 타입 힌트를 주려면 ??
`fun:int = 1` 같이 매개변수를 받지 않는 경우는 편하지만 `fun:int $n:int` 로 하면 이상하지 않을까?
않으면 정의 전에 하스켈처럼 선언할대 타입 힌트를 줄 수 있게 할까?
```
fun:int
fun 3 2 = "hi" // Compile error!
fun $a $b = 3
```

## 타입

타입 신택스라던지 실제 사용이라던지 어떻게 해야할지 모르겠다.....
일단 타입은 최소의 namedtuple 수준으로 생각하고 싶다. 멤버 메서드같은것도 있으면 좋을까?

```
type Person(name:string, age:int) {
    .say = print name
}
```

## 신택스 철학

철학 이전에 생각해야 할것은 연산자(?) 결합 순위이다. 패턴을 호출하는데 어떤게 먼저 결합해야할지 순서가 정확히 없으면 진짜 개판이 될 텐데..

굳이 람다 대수처럼 괄호를 안쓰는 방향을 지향할 필요가 있을까 ..? 타입 생성자가 괄호를 사용하는데 이는 structing 을 위해 특별한 케이스로 봐도 괜ㅊ낳을거띾

아님 함수(패턴)을 선언할 때 굳이 괄호를 쓰고 싶다면 **지금처럼 한다면** 이렇게 써도 되긴 하지

```
fib `( 0 `) = 1
fib `( 1 `) = 1
fib `( $n:int `) = fib(n-1) + fib(n-2)
```

그치만 이렇게 하면 익숙하긴 하지만 .... 
그리고 괄호를 안쓰거나 쓰는 쪽이 막 섞이면 안돼니까 아마도 한쪽으로 방향을 아예 정해야 할듯.

그치만 저거 괄호를 강제하지 않아도 결국 괄호를 쓰는 경우는 쓸테니까 그러면 복잡해지려나 ??

```
// print(a(1+1, -b(c), d) 는 다음과 같다
print ((a (1+1)) -(b c)) d

// 한눈에 봐도 괄호와 콤마로 구분하는게 더 알아보기 쉽잖아??
```

하스켈 같은 경우는 `.` 와 `$` 신택스가 있어서 저런 경우는 괄호가 많지 않아도 잘 표현할 수 있었는데 여기서는 베이스 문법에서 그런건 제공하지 않을 거니까 어떻게 해야할지 모르겟다 ....

## 메서드 정의 패턴

기차에서 좋은 생각이 났다!! 메서드를 정의하는 패턴을 정의하는 것이다. 예를 들어 `fn` 패턴이라 하자.

```
type Method(name, params, inner) {
    .ctor = {
        $name $params = { $inner }
    }
    `( given `) = name given
}

fn $name `( `) `{ $inner `} = { 
    Method($name, [], $inner)
    // 이거 스코프가 밖까지 나가야함. ..,,, 패턴 내에서 패턴 생성 어뜨케 할까 ?
    // 라고 처음엔 생각했지만 약간 생각이 바뀜. 이거 좀 쩔어
    // Method의 생성자에서 name params 의 패턴이 생성되고 이게 멤버 변수임
    // 그리고 Method를 호출하는 두번째 패턴에서 그걸 사용하는거지
}

fn $name `( $first(`:$ftyp:type)? (`, ($exprs(`:$typ:type)?)* `) `{ $inner `} = {
    Method($name, [$first`:$ftyp, *[($exprs`:$typ)*]], $inner)
}

fn say(msg:string, from:string) {
    print(from, ": ", msg)
}

say("Hey!", "John") // 요로코롬 호출 가능함 \(>ㅡ<)/
```

## 파라미터 -> expression/value 매칭?

파라미터를 ast로 받아서 메타 프로그래밍이 가능해야 한다.
아마도 파라미터를 ast로 받느냐 아니면 그 값을 받느냐 둘을 구분하는 무언가가 있어야 할 것 같다.

파라미터를 식 트리로 받는 간단한 예. 사실 러스트에서 보고 만든게 맞음.

```
log $val:expr = print $val.tostr " : " $val.val

log (1 + 1) // print "(1 + 1) : 2"
```

그치만 이걸 이렇게 받으면 당근 문제가 생김.

```
ho $n:expr = print "epr"
ho $n:obj = print "val"
```

ho 패턴에 어떤 값을 넣던 어떤 게 먼저 실행될까 ,, ? 아마 이거는 먼저 선언된 패턴 위주로 실행될 테지만 그래도 이거 되게 괴로울 것 같다.

그냥 이런 코드를 쓰면 안된다 하자

## 패턴내에서 패턴의 스코프

요거는 ~~~~ 다음 시간에 생각해보도록 하자~~~^^