namespace ConsiderConvertingMethodToProperty

module Instance =
  let mutable isRunner = false
  let mutable isOpenCover = false

  let isOpenCoverRunner() = isOpenCover && isRunner