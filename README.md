# DrNet.Hyperloop
DrNet.Hyperloop library helps to rewrites recursive iterators to non-quadratic recursive iterators with minimal code refactory.

As a result, the execution time will be reduced from quadratic to linear dependence on the recursion depth:
![image](https://user-images.githubusercontent.com/18285074/91877833-85855980-ec4c-11ea-90fb-d8764a1c2171.png)

See ['All About Iterators'](https://docs.microsoft.com/en-us/archive/blogs/wesdyer/all-about-iterators) article.

It can be used by compiler to implement [Non-quadratic Recursive Iterators feature request](https://github.com/dotnet/csharplang/issues/378).

You can use the DrNet.Hyperloop library to improve the performance
1. Recursive iterators.
2. Concatinated iterators.
3. Deeply nested iterators even if there is no recursive nesting between them.
