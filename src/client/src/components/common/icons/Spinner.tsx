function Spinner({
  h = 8,
  w = 8,
}: {
  h?: number | string;
  w?: number | string;
}) {
  return (
    <div className="flex justify-center mt-20">
      <div
        className="animate-spin rounded-full border-b-2 border-neutral-600"
        style={{ height: h, width: w }}
      />
    </div>
  );
}

export default Spinner;
