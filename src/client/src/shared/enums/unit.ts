export enum Unit {
  Default,
  milliliter,
  grams,
  liter,
  kilogram,
  teaspoon,
  tablespoon,
}

export const unitLabels = {
  [Unit.Default]: "",
  [Unit.milliliter]: "мл",
  [Unit.grams]: "г",
  [Unit.liter]: "л",
  [Unit.kilogram]: "кг",
  [Unit.teaspoon]: "ч.л.",
  [Unit.tablespoon]: "ст.л.",
};
