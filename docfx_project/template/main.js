// Po załadowaniu DocFX: usuń z górnego paska zagnieżdżone podmenu (level2/level3).
(function () {
  function flattenNavbar() {
    var $root = $('#navbar > ul.navbar-nav');
    if (!$root.length) return;
    $root.find('ul').remove();
    $root.find('.expand-stub').remove();
    $root.children('li').each(function () {
      var $a = $(this).children('a').first();
      if ($a.length && !$a.attr('href')) {
        $(this).remove();
      }
    });
  }

  $(function () {
    flattenNavbar();
    setTimeout(flattenNavbar, 300);
    setTimeout(flattenNavbar, 1000);
  });
})();
