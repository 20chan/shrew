# Shrew

언어의 목표는 모든걸 프로시저로 생각하는 LISP와 비슷함. 모든 것을 패턴으로 생각하자

 - [ ] 언어 설계
    - [ ] 언어 철학 설계
    - [ ] 예제 코드 작성
    - [ ] 언어 문법 구체화
        - [ ] 패턴
            - [ ] 패턴의 작동 원리
            - [ ] ast 파라미터 넘기는거
            - [ ] 매개변수의 반복/정규식 표현
            - [ ] **패턴 결합 순서**
        - [ ] 타입 시스템
        - [ ] 메모리
        - [ ] 스레드
        - [ ] 패키지
 - [ ] 컴파일러 제작
     - [ ] 언어 오토마타 설계
     - [ ] 렉서
     - [ ] 파서
     - [ ] 인터프리터
     - [ ] 바이트코드 변환
     - [ ] VM

## 느낌적인느낌느낌

### Hello World

```
main = print("Hello World")
```

### Sum1To10

```
main = {
    sum = 0
    i = 1
    while(i <= 10) {
        sum += i
    }
    print(sum)
}
```

### Fibonacci Sequence

```
// recursive
fib(0) = 1
fib(1) = 1
fib($n:int) = fib($n-1) + fib($n-2)
```

```
// iterative
fib($n:int) = {
    a = 0
    b = 1
    loop($n) {
        a, b = b, a + b
    }
    b
}
```

### Define `for`

```
for ( $init; $cond; $after; ) { $inner } = {
        $init
        while($cond) {
            $inner
            $after
        }
}

for (i = 0; i<=10; i++) {
    print(i)
}
```

### Define Array, Range Syntax

```
arr [ ] = array()
arr [ $first $(`, $elem)*@elems ] = array($first $elems*)
// arr[1, 2, 3] = array(1, 2, 3)
// arr[1, 1+1, 1] = array(1, 1+1, 1)

$from `... $to = range $from $to
```

### Type

```
type Person(name:string, age:int)

a = Person("0xad", 3)
a.name = a.name + ";"
print(a.name) // "0xad;"
```

멤버 메서드 ?

```
type Person(name:string, age:int) {
    .old() = {
        age = age + 1
    }
}

a = Person("0xad", 3)
a.old()
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
fun(3, 2) = "hi" // Compile error!
fun($a, $b) = 3
```

## 타입

타입 신택스라던지 실제 사용이라던지 어떻게 해야할지 모르겠다.....
일단 타입은 최소의 namedtuple 수준으로 생각하고 싶다. 멤버 메서드같은것도 있으면 좋을까?

```
type Person(name:string, age:int) {
    .say() = print(name)
}
```

## 신택스 철학

철학 이전에 생각해야 할것은 연산자(?) 결합 순위이다. 패턴을 호출하는데 어떤게 먼저 결합해야할지 순서가 정확히 없으면 진짜 개판이 될 텐데..

강제는 아니지만 이런 규칙을 사용하자.
일반적으로 변수로 사용되는 패턴은 이름만 있을 테고 메서드처럼 사용되는 패턴은 이름 뒤에 괄호로 시작하고 괄호로 끝나야 함.

```
// RECOMMENDED
a = 0
a([]) = 0
a{} = 0

// NOT RECOMMENDED
a([)] = 1
a 1 = 1
```


## 파라미터 -> expression/value 매칭?

파라미터를 ast로 받아서 메타 프로그래밍이 가능해야 한다.
아마도 파라미터를 ast로 받느냐 아니면 그 값을 받느냐 둘을 구분하는 무언가가 있어야 할 것 같다.

파라미터를 식 트리로 받는 간단한 예. 사실 러스트에서 보고 만든게 맞음.

```
log($val:expr) = print $val.tostr " : " $val.val

log(1 + 1) // print "1 + 1 : 2"
```

그치만 이걸 이렇게 받으면 당근 문제가 생김.

```
ho($n:expr) = print("epr")
ho($n:obj) = print("val")
```

ho 패턴에 어떤 값을 넣던 어떤 게 먼저 실행될까 ,, ? 아마 이거는 먼저 선언된 패턴 위주로 실행될 테지만 그래도 이거 되게 괴로울 것 같다.

그냥 이런 코드를 쓰면 안된다 하자

## 패턴내에서 패턴의 스코프

요거는 ~~~~ 다음 시간에 생각해보도록 하자~~~^^