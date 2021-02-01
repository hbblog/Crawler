# C#爬虫
 现在爬到的是https://www.qiushibaike.com/text网址内容，包含所有页内数据
 数据库是sqlserver2014，数据库表结构在\WeChatTest\App_Data\DB_QiuBaiHappy.txt
# 大致流程
## 1.将想要抓取的页面读取到内存
## 2.加载页面
## 3.有分页的读取分页标签，查询一共有多少页（XPath）
## 4.组合分页URL
## 5.抓取当前页的内容（XPath）
## 6.保存到数据库
## 7.微信小程序调用数据读取接口，显示抓取到的内容
