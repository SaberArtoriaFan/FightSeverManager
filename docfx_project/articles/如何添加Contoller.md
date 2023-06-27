# 如何添加Controller指令
## 1.明确划分
首先需要知道该指令是[R-M]还是[M-L]\
*[R-M]即后端与管理服务器通讯，[M-L]即管理服务器与本地实例通讯。*
## 2.添加函数
根据类型不同选择在**ClientServerController**或是**LocalController**类中添加相应**ActionCode**名字的函数方法，该函数的参数与返回值请与该类中的其他函数保持一致。

如此就做到了添加指定通讯对象的具体**ActionCode**响应。


