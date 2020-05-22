// pages/newlist/newlist.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    topBars: [{
        id: 1,
        name: "段子"
      },
      {
        id: 2,
        name: "热图"
      },
      {
        id: 3,
        name: "视频"
      },
      // {
      //   id: 4,
      //   name: "直播"
      // },
      // {
      //   id: 5,
      //   name: "影视"
      // },
      // {
      //   id: 6,
      //   name: "游戏"
      // },
      // {
      //   id: 7,
      //   name: "新闻"
      // }
    ],
    selectType: 1,
    newsList: [
      //  {
      //   id: 1,
      //   userName: "HB",
      //   headUrl: "/pages/resource/头像.jpg",
      //   isFocus: true,
      //   newsTitle: "只争朝夕，不负韶华",
      //   isVideo: true,
      //   videoUrl: "/pages/resource/test.mp4",
      //   newsText: null,
      //   newAbstrack: null,
      //   imageUrl: null,
      //   isOriginal: true,
      //   createTime: "2020-04-29T09:45:15",
      // }, {
      //   id: 2,
      //   userName: "HB",
      //   headUrl: "/pages/resource/title1.jpg",
      //   isFocus: false,
      //   newsTitle: "",
      //   isVideo: false,
      //   videoUrl: null,
      //   newsText: "尼日尔东下回合，欧拉函数那地方的省份许我天若有情拉看法是在考虑现代化反好",
      //   newAbstrack: "简介",
      //   imageUrl: "/pages/resource/title1.jpg",
      //   isOriginal: false,
      //   createTime: "2020-04-29T10:45:15",
      // }
    ],
  },

  selectBar(e) {
    var typeid = e.currentTarget.dataset.typeid;
    this.setData({
      selectType: typeid
    });
  },
  getNewsList() {
    var that = this;
    wx.request({
      url: 'http://10.185.1.51:8084/',
      success(res) {
        that.setData({
          newsList: res.data,
        })
      }
    })
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.getNewsList();
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  }
})