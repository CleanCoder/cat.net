# 改动点：
* 支持异步调用；原先CAT上下文使用ThreadLocal来保持，改进后使用了AsyncLocal
* 支持异步并行，并提供TransactionUtil.WrapWithForkedTransactionAsync封装方法
* 支持从Config中读取server的配置信息(CatConfigurationSection)
* 数据库访问Wrap类(DBUtil), 帮助在业务代码前后开启和结束Transaction
