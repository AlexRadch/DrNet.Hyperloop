# DrNet.Hyperloop
DrNet.Hyperloop library helps to rewrites recursive iterators to linear execution time dependence on the recursion depth with minimal code refactory.

As a result, the execution time will be reduced from quadratic to linear dependence on the recursion depth:
![image](https://user-images.githubusercontent.com/18285074/91877833-85855980-ec4c-11ea-90fb-d8764a1c2171.png)

See ['All About Iterators'](https://docs.microsoft.com/en-us/archive/blogs/wesdyer/all-about-iterators) article.

It can be used by compiler to implement [Non-quadratic Recursive Iterators feature request](https://github.com/dotnet/csharplang/issues/378).