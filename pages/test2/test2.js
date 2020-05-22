// pages/test2/test2.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    msg: "shuju",
    msg2: "<b>hello <p style='color:red;'>world</p></b>"
  },

  clickme: function (event) {
    this.data.msg = "你好";
    this.setData({
      msg: "你好"
    });
    console.log(this.data.msg);
    //console.log(event.currentTarget.dataset);
    //console.log(event.currentTarget.dataset.val2);
  },
  changeme() {
    console.log(this.data.msg);
  },
  getUserInfo(e) {
    console.log(e);
  },
  getPhoneNumber(e) {
    console.log(e.detail.errMsg)
    console.log(e.detail.iv)
    console.log(e.detail.encryptedData)
  },
  getContact(e) {
    console.log(e);
  },

  shouAlert() {
    // wx.showToast({
    //   title: '加载中...',
    //   icon:"success",//"loading"//success//none
    //   duration:5000
    // })
    // wx.showLoading({
    //   title: '加载中...',
    //   //不写时间就会一直存在
    // })
    // setTimeout(function(){
    //   wx.hideLoading();
    // },2000)
    wx.showModal({
      title: "提示",
      content: "模式弹窗",
      success(res) {
        if (res.confirm) {
          console.log(1);
        }
        if (res.cancel) {
          console.log(0);
        }
      }
    })
  },
  navi() {
    wx.navigateTo({
      url: '/pages/test/test?id=1',
    })
  },
  getData() {
    var that = this;
    wx.request({
      url: 'http://localhost:52670/ashx/IsLogin.ashx?isLogin=1',
      success(res){
        var data = res.data;
        console.log(data);
        that.setData({
          result:data
        })
        console.log(that.data.result);
      }
    })
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

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