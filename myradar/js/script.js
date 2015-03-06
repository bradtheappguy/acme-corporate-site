/*
 * All the plugins init are in this file
 *
 */
var map;
$(document).ready(function() {

  // Scroll to top
  $.scrollUp({
     scrollText: '', // Text for element
  });
  
  // activate carousels
  $('#mobile-carousel').carousel();
  $('#testimonials-carousel').carousel();
  
  // init fitvids plugin
  $(".video").fitVids();


});